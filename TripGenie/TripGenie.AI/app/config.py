from pydantic_settings import BaseSettings, SettingsConfigDict
from pydantic import Field

class Settings(BaseSettings):
    anthropic_api_key: str = Field(..., validation_alias="ANTHROPIC_API_KEY")
    shared_secret: str = Field(..., validation_alias="SHARED_SECRET")
    client_id: str = Field("tripgenie-core", validation_alias="CLIENT_ID")
    host: str = Field("0.0.0.0", validation_alias="HOST")
    port: int = Field(8000, validation_alias="PORT")
    redis_url: str = Field("redis://redis:6379/0", validation_alias="REDIS_URL")  # default to redis container hostname for compose
    claude_model: str = Field("claude-3-5-sonnet-20241022", validation_alias="CLAUDE_MODEL")

    model_config = SettingsConfigDict(
        env_file=".env",
        env_file_encoding="utf-8",
        extra="ignore"
    )

settings = Settings()
