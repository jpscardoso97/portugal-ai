# Portugal-AI

[![CodeFactor](https://www.codefactor.io/repository/github/jpscardoso97/portugal-ai/badge)](https://www.codefactor.io/repository/github/jpscardoso97/portugal-ai)
[![.NET App Pipeline](https://github.com/jpscardoso97/portugal-ai/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jpscardoso97/portugal-ai/actions/workflows/dotnet.yml)
[![Python API](https://github.com/jpscardoso97/portugal-ai/actions/workflows/python.yml/badge.svg)](https://github.com/jpscardoso97/portugal-ai/actions/workflows/python.yml)
[![Docker Build](https://github.com/jpscardoso97/portugal-ai/actions/workflows/docker.yml/badge.svg)](https://github.com/jpscardoso97/portugal-ai/actions/workflows/docker.yml)

## Project Description

Portugal AI is an AI project that aims to provide a useful user interface for tourists visiting Portugal. The main goal is to not only provide information about the most popular places to visit in Portugal, but to provide suggestions based on curated expertize by locals.  
Using a chatbot-like interface, the user can ask for suggestions based on their preferences, such as the type of food they like, the type of activities they enjoy, and the type of places they like to visit.  

## System Architecture

<img width="823" alt="arch" src="https://github.com/jpscardoso97/portugal-ai/assets/29354431/72ca91df-384e-46a3-970a-4d3d71aa8c51">

## Usage examples/screenshots

![demo](https://github.com/user-attachments/assets/a27f5907-7fce-4b4b-8ab9-90813a7618d2)

## How to run
- Clone the repository
- Download Meta-Llama-3-8B-Instruct llamafile from [here](https://huggingface.co/Mozilla/Meta-Llama-3-8B-Instruct-llamafile/resolve/main/Meta-Llama-3-8B-Instruct.Q5_K_M.llamafile?download=true)
- Make file executable: 
  ```
  chmod +x Meta-Llama-3-8B-Instruct.Q5_K_M.llamafile
  ```
- Run the following command to serve the model locally: 
  ```
  ./Meta-Llama-3-8B-Instruct.Q5_K_M.llamafile
  ```
- Run application by running docker-compose in the repo root folder. This will run both app containers and bootstrap the vector database automatically:
  ```
  docker-compose up
  ```
- Access the app user interface at `http://localhost:5001`

## Performance/Evaluation

The speed performance of the application was measured by calculating the average response time of the system for a series of pre-defined prompts.  
Here are the results:  

| Query | Avg. Response Time |
|-------|--------------------|
| 1     |                    |
| 2     |                    |
| 3     |                    |
| 4     |                    |
| 5     |                    |
| 6     |                    |

For the system accuracy performance evaluation, it was compared against the original LLM (Llama-3 8B without RAG), ChatGpt-4o and Claude 3.5 Sonnet.
The overall result was very satisfactory for the implemented system. It outperforms all the competitors except ChatGPT-4o because of it's capacity to use curated content as context to its responses. Even though the SOTA chat models provide richer responses, most of the information in such responses is not accurate (specially for locations that are not big cities like Porto or Lisbon). Most of the recommended restaurants in medium/small sized cities like Aveiro or Coimbra either don't exist, are innacurately described or are not located where the model says they are.   
To measure and compare such accuracy, a scoring system was used against a set of 10 questions about 5 different locations that are supported by Portugal AI (meaning that they are included in the system's vector database). The score for each response goes from 1-4 being 1 a totally innacurate response (restaurants don't exist or have no relation to the question) or refuses to respond, to 4 which is a perfect recommendation (the restaurant/s exist/s, are in the correct location, and are well described/according to the question).   

The results are detailed in the table below:

|        | Location | Query                                                                        | Portugal AI | Llama-3 8B | ChatGPT-4o | Claude 3.5 Sonnet |
|--------|----------|------------------------------------------------------------------------------|-------------|------------|------------|-------------------|
|   -    | Aveiro   | Where should I eat in Aveiro?                                                |     4.0     |    1.0     |     4.0    |        4.0        |
|   -    | Aveiro   | What are the recommended restaurants in Aveiro to eat Sushi?                 |     4.0     |    1.0     |     3.5    |        1.0        |
|   -    | Porto    | Where should I eat in Porto?                                                 |     1.0     |    4.0     |     3.5    |        4.0        |
|   -    | Porto    | What are the recommended restaurants in Porto to eat Francesinha?            |     1.0     |    2.5     |     4.0    |        3.5        |
|   -    | Lisbon   | Where should I eat in Lisbon?                                                |     4.0     |    3.5     |     4.0    |        4.0        |
|   -    | Lisbon   | What are the recommended restaurants in Lisbon to eat Brazilian food?        |     2.0     |    1.5     |     3.5    |        2.0        |
|   -    | Coimbra  | Where should I eat in Coimbra?                                               |     4.0     |    2.5     |     3.5    |        4.0        |
|   -    | Coimbra  | What are the recommended restaurants in Coimbra to eat local dishes?         |     4.0     |    1.0     |     4.0    |        4.0        |
|   -    | Viseu    | Where should I eat in Viseu?                                                 |     4.0     |    1.0     |     4.0    |        1.0        |
|   -    | Viseu    | Can you suggest a cosy restaurant with portuguese traditional food in Viseu? |     4.0     |    1.0     |     4.0    |        1.0        |
| Total  |          |                                                                              |     32.0    |    19.0    |     38.0   |        28.0         |

## CI/CD
There are three pipelines in this project:
- [.NET App Pipeline](https://github.com/jpscardoso97/portugal-ai/actions/workflows/dotnet.yml): builds and tests the .NET main application.
- [Python API Pipeline](https://github.com/jpscardoso97/portugal-ai/actions/workflows/python.yml): builds and tests the Python RAG API.
- [Docker Pipeline](https://github.com/jpscardoso97/portugal-ai/actions/workflows/docker.yml): builds and pushes app's Docker images to Dockerhub.

Additionally, the repository is analysed by [CodeFactor](https://www.codefactor.io/repository/github/jpscardoso97/portugal-ai) which performs static code analysis on every commit. Currently configured tools:
- Hadolint: AST-based linter for building best practice Docker images.
- YamlLint: Checks for syntax validity, weirdnesses like key repetition and cosmetic problems.
- Trivy: Spots potential security issues
- StyleLint: Helps to avoid errors and enforce consistent conventions in CSS and HTML.
- StyleCop Analyzers: Analyzes C# code to enforce a set of style and consistency rules.


