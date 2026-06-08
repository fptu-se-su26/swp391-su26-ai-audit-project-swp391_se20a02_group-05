from abc import ABC, abstractmethod
from dataclasses import dataclass, field
from typing import Any
import os


@dataclass
class CodeSample:
    file_content: list[str] = field(default_factory=list)
    file_names: list[str] = field(default_factory=list)


@dataclass
class CodeSamplingOptions:
    # Increased from 10/100 — smart-sampler fills these slots with critical-path
    # files first, so more budget is consumed usefully.
    max_files: int = 20
    max_lines_per_file: int = 150
    extensions: list[str] = field(default_factory=list)


# ---------------------------------------------------------------------------
# Critical-path constants
# ---------------------------------------------------------------------------

# Tier 1 — entry-point filenames (exact lower-case match, any directory depth)
_ENTRY_POINT_NAMES: frozenset = frozenset({
    "main.py", "app.py", "server.py", "asgi.py", "wsgi.py", "run.py",
    "main.ts", "app.ts", "server.ts", "index.ts",
    "main.js", "app.js", "server.js", "index.js",
    "program.cs", "startup.cs",
    "main.java", "application.java",
    "main.go",
    "main.rs",
    "main.rb", "application.rb",
    "main.php", "index.php",
})

# Tier 2 — directory-name fragments that indicate business logic
_CRITICAL_PATH_DIRS: frozenset = frozenset({
    "services", "service",
    "controllers", "controller",
    "handlers", "handler",
    "usecases", "use_cases", "usecase",
    "repositories", "repository",
    "domain",
    "core",
    "middleware",
    "routes", "routers", "routing",
    "api",
    "business",
    "features",
})

_EXCLUDED_DIRS: frozenset = frozenset({
    ".git", "node_modules", "bin", "obj", "dist", "vendor",
    "venv", "packages", "__pycache__", ".venv", "coverage",
    "migrations",  # usually boilerplate
})

_PACKAGE_NAMES: frozenset = frozenset({
    "package.json", "requirements.txt", "go.mod", "pom.xml",
    "cargo.toml", "docker-compose.yml",
})

_DOC_NAMES: frozenset = frozenset({
    "readme.md", "architecture.md", "contributing.md",
})

_CODE_EXTENSIONS: frozenset = frozenset({
    ".py", ".js", ".ts", ".tsx", ".cs", ".java", ".go",
    ".rs", ".rb", ".php",
})


def _is_in_critical_dir(rel_path: str) -> bool:
    """Return True if any path segment matches a critical-path directory name."""
    parts = rel_path.replace("\\", "/").split("/")
    # parts[-1] is the filename — check all directory segments
    for segment in parts[:-1]:
        if segment.lower() in _CRITICAL_PATH_DIRS:
            return True
    return False


class ICodeSampler(ABC):
    @abstractmethod
    async def sample_async(self, repo: Any, token: str, options: CodeSamplingOptions) -> CodeSample:
        ...


class CodeSampler(ICodeSampler):
    """
    Smart critical-path sampler.

    File selection priority (fills ``options.max_files`` slots top-down):
        Tier 1 — entry-point files  (main.py, Program.cs, index.ts …)
        Tier 2 — files inside critical-path directories  (services/, controllers/ …)
        Tier 3 — package / manifest files  (package.json, requirements.txt …)
        Tier 4 — documentation files  (README.md, architecture.md …)
        Tier 5 — remaining code files, largest-first (previous behaviour)

    Each file is truncated to ``options.max_lines_per_file`` lines.
    Repository hard-limits (10 000 files / 150 MB) are enforced before selection.
    """

    async def sample_async(self, repo: Any, token: str, options: CodeSamplingOptions) -> CodeSample:
        repo_path = str(repo)

        # ------------------------------------------------------------------
        # Walk the repo once, bucket every file into a priority tier.
        # ------------------------------------------------------------------
        tier1_entry: list[tuple[str, str]] = []        # (rel_path, abs_path)
        tier2_critical: list[tuple[str, str, int]] = []  # + size
        tier3_package: list[tuple[str, str]] = []
        tier4_doc: list[tuple[str, str]] = []
        tier5_remaining: list[tuple[str, str, int]] = []

        total_files = 0
        total_size = 0

        for root, dirs, files in os.walk(repo_path):
            dirs[:] = [d for d in dirs if d not in _EXCLUDED_DIRS]

            for f in files:
                total_files += 1
                full_path = os.path.join(root, f)
                try:
                    size = os.path.getsize(full_path)
                    total_size += size
                except OSError:
                    size = 0

                rel_path = os.path.relpath(full_path, repo_path)
                lower_f = f.lower()
                _, ext = os.path.splitext(lower_f)

                if lower_f in _PACKAGE_NAMES or f.endswith(".csproj"):
                    tier3_package.append((rel_path, full_path))
                elif lower_f in _DOC_NAMES:
                    tier4_doc.append((rel_path, full_path))
                elif ext in _CODE_EXTENSIONS:
                    if lower_f in _ENTRY_POINT_NAMES:
                        tier1_entry.append((rel_path, full_path))
                    elif _is_in_critical_dir(rel_path):
                        tier2_critical.append((rel_path, full_path, size))
                    else:
                        tier5_remaining.append((rel_path, full_path, size))

        if total_files > 10_000:
            raise Exception("Repository exceeds the maximum limit of 10,000 files.")
        if total_size > 150 * 1024 * 1024:
            raise Exception("Repository exceeds the maximum limit of 150 MB in size.")

        # Sort Tier 2 and Tier 5 by descending file size (bigger = more logic)
        tier2_critical.sort(key=lambda x: x[2], reverse=True)
        tier5_remaining.sort(key=lambda x: x[2], reverse=True)

        # ------------------------------------------------------------------
        # Fill selection budget respecting priority order.
        # ------------------------------------------------------------------
        selected: list[tuple[str, str]] = []
        seen: set[str] = set()

        def _add(rel: str, full: str) -> bool:
            if len(selected) >= options.max_files:
                return False
            if rel in seen:
                return True
            seen.add(rel)
            selected.append((rel, full))
            return True

        for rel, full in tier1_entry:
            _add(rel, full)

        for rel, full, _ in tier2_critical:
            if len(selected) >= options.max_files:
                break
            _add(rel, full)

        for rel, full in tier3_package:
            if len(selected) >= options.max_files:
                break
            _add(rel, full)

        for rel, full in tier4_doc:
            if len(selected) >= options.max_files:
                break
            _add(rel, full)

        for rel, full, _ in tier5_remaining:
            if len(selected) >= options.max_files:
                break
            _add(rel, full)

        # ------------------------------------------------------------------
        # Read selected files, truncating to max_lines_per_file.
        # ------------------------------------------------------------------
        def read_truncated(path: str, max_lines: int) -> str:
            lines: list[str] = []
            try:
                with open(path, "r", encoding="utf-8", errors="ignore") as fh:
                    for _ in range(max_lines):
                        line = fh.readline()
                        if not line:
                            break
                        lines.append(line)
            except Exception:
                return "[Error reading file]"
            return "".join(lines)

        file_names: list[str] = []
        file_contents: list[str] = []
        for rel, full in selected:
            file_names.append(rel)
            file_contents.append(read_truncated(full, options.max_lines_per_file))

        return CodeSample(file_content=file_contents, file_names=file_names)
