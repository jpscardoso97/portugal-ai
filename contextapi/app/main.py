from app.services.vector_search_svc import VectorSearchService

from fastapi import FastAPI, Depends

app = FastAPI()


@app.get("/")
async def root():
    return {"message": "Hello World"}

@app.get("/search")
async def search(query: str, location: str = None, search_svc=Depends(VectorSearchService)):
    return search_svc.search_vector(query, location)
