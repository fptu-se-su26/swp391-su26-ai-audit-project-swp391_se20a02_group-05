import json
import re
from typing import Any


class LlmResponseParser:
    def extract_json_block(self, response: str) -> Any:
        match = re.search(r"```(?:json)?\s*(\{.*?\}|\[.*?\])\s*```", response, re.DOTALL)
        if match:
            return json.loads(match.group(1))
        match = re.search(r"(\{.*\}|\[.*\])", response, re.DOTALL)
        if match:
            return json.loads(match.group(1))
        return {}

    def is_valid_json(self, text: str) -> bool:
        try:
            json.loads(text)
            return True
        except (json.JSONDecodeError, ValueError):
            return False
