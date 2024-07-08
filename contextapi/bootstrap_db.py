import os

from pymilvus import model, MilvusClient

client = MilvusClient("portugal_ai.db")
COLLECTION_NAME = "portugal_ai"

if client.has_collection(collection_name=COLLECTION_NAME):
    client.drop_collection(collection_name=COLLECTION_NAME)
client.create_collection(
    collection_name=COLLECTION_NAME,
    dimension=384,
)

embedding_fn = model.dense.SentenceTransformerEmbeddingFunction(
    model_name='all-MiniLM-L6-v2',
    device='cpu'
)

docs = []
doc_locations = []

# Iterate locations
for location in os.listdir("data"):
    # Iterate files inside location folder
    for file in os.listdir(f"data/{location}"):
        with open(f"data/{location}/{file}", "r") as f:
            content = f.read()
            docs.append(content)
            doc_locations.append(location)  # Save the location of the document
            

vectors = embedding_fn.encode_documents(docs)

data = [
    {"id": i, "vector": vectors[i], "text": docs[i], "location": doc_locations[i]}
    for i in range(len(vectors))
]

print("Data has", len(data), "entities, each with fields: ", data[0].keys())

res = client.insert(collection_name=COLLECTION_NAME, data=data)


# Test the search
print("Testing database search...")
query_vectors = embedding_fn.encode_queries(["I'm looking for a francesinha, where should I eat?"])

res = client.search(
    collection_name=COLLECTION_NAME,  # target collection
    data=query_vectors,  # query vectors
    filter="location == 'braga'",  # filter results
    limit=1,  # number of returned entities
    output_fields=["text", "location"],  # specifies fields to be returned
)

for rec in res[0]:
    print(f"Distance: {rec['distance']}")
    print(f"Location: {rec['entity']['location']}")
    print(f"Text: {rec['entity']['text'].splitlines()[0]}")
    print("----")