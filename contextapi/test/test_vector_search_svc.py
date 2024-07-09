import pytest
from unittest.mock import patch, MagicMock
import sys
import os

# Add the directory containing 'app' to the Python path
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

from app.services.vector_search_svc import VectorSearchService
from app.models.search_result import SearchResult

@pytest.fixture
def mock_services():
    with patch('app.services.vector_search_svc.MilvusClient') as MockMilvusClient, \
         patch('app.services.vector_search_svc.model.dense.SentenceTransformerEmbeddingFunction') as MockEmbeddingFunction:
        mock_client = MockMilvusClient.return_value
        mock_embedding_fn = MockEmbeddingFunction.return_value
        service = VectorSearchService()
        yield mock_client, mock_embedding_fn, service

def test_search_vector_no_location(mock_services):
    mock_client, mock_embedding_fn, service = mock_services
    query = "I'm looking for a francesinha, where should I eat?"
    mock_query_vector = [[0.1, 0.2, 0.3]]
    mock_embedding_fn.encode_queries.return_value = mock_query_vector
    mock_search_result = [{
        'distance': 0.1,
        'entity': {
            'text': 'Try the restaurant down the street.',
            'location': 'Porto'
        }
    }]
    mock_client.search.return_value = [mock_search_result]

    results = service.search_vector(query)

    mock_embedding_fn.encode_queries.assert_called_once_with([query])
    mock_client.search.assert_called_once_with(
        collection_name="portugal_ai",
        data=mock_query_vector,
        filter=None,
        limit=1,
        output_fields=["text", "location"]
    )
    assert len(results) == 1
    assert results[0].text == 'Try the restaurant down the street.'
    assert results[0].location == 'Porto'

def test_search_vector_with_location(mock_services):
    mock_client, mock_embedding_fn, service = mock_services
    query = "I'm looking for a francesinha, where should I eat?"
    location = "Porto"
    mock_query_vector = [[0.1, 0.2, 0.3]]
    mock_embedding_fn.encode_queries.return_value = mock_query_vector
    mock_search_result = [{
        'distance': 0.1,
        'entity': {
            'text': 'Try the restaurant down the street.',
            'location': 'Porto'
        }
    }]
    mock_client.search.return_value = [mock_search_result]

    results = service.search_vector(query, location)

    mock_embedding_fn.encode_queries.assert_called_once_with([query])
    mock_client.search.assert_called_once_with(
        collection_name="portugal_ai",
        data=mock_query_vector,
        filter="location == 'Porto'",
        limit=1,
        output_fields=["text", "location"]
    )
    assert len(results) == 1
    assert results[0].text == 'Try the restaurant down the street.'
    assert results[0].location == 'Porto'

def test_search_vector_exception(mock_services):
    mock_client, mock_embedding_fn, service = mock_services
    query = "I'm looking for a francesinha, where should I eat?"
    mock_embedding_fn.encode_queries.side_effect = Exception("Embedding error")

    results = service.search_vector(query)

    assert results == []