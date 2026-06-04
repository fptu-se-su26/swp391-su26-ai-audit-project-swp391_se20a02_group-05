import json
from abc import ABC, abstractmethod
from typing import Any, Type, TypeVar

T = TypeVar("T")


class IStructuredOutputParser(ABC):
    @abstractmethod
    def parse(self, response: str, target_type: Type[T]) -> T:
        ...


class JsonSchemaValidator(IStructuredOutputParser):
    def parse(self, response: str, target_type: Type[T]) -> T:
        # Implementation will go here - validate and parse JSON
        data = json.loads(response)
        return target_type(**data) if isinstance(data, dict) else data
