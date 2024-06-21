import os
from contextapi.models.search_result import SearchResult
from pymilvus import MilvusClient, model

COLLECTION_NAME = "portugal_ai"
CURR_PATH = os.path.dirname(os.path.realpath(__file__))
DB_PATH = CURR_PATH+"../portugal_ai.db"

class VectorSearchService:

    def __init__(self):
        self.client = MilvusClient("portugal_ai.db")

        self.embedding_fn = model.dense.SentenceTransformerEmbeddingFunction(
            model_name='all-MiniLM-L6-v2',
            device='cpu'
        )

    def search_vector(self, query, location=None) -> [SearchResult]:
        print(f"Query: {query}")
        print(f"Location: {location}")

        #query_vectors = self.embedding_fn.encode_queries([query])
        query_vectors = self.embedding_fn.encode_queries(["I'm looking for a francesinha, where should I eat?"])

        query_filter = None if location is None else f"location == '{location}'"

        try:
            search_res = self.client.search(\
                collection_name=COLLECTION_NAME,  # target collection
                data=query_vectors,  # query vectors
                filter=query_filter,  # filter results
                limit=1,  # number of returned entities
                output_fields=["text", "location"],  # specifies fields to be returned
            )
        except Exception as e:
            print(f"Error: {e}")
            return []

        results = []

        for rec in search_res[0]:
            print(f"Distance: {rec['distance']}")
            print(f"Location: {rec['entity']['location']}")
            print(f"Text: {rec['entity']['text'].splitlines()[0]}")
            print("----")

            results.append(SearchResult(text=rec['entity']['text'], 
                             location=rec['entity']['location']))
            
        return results
        