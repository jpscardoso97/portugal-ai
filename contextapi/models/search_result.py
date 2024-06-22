from pydantic import BaseModel

class SearchResult(BaseModel):
    text: str
    location: str