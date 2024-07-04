# Portugal-AI

[![CodeFactor](https://www.codefactor.io/repository/github/jpscardoso97/portugal-ai/badge)](https://www.codefactor.io/repository/github/jpscardoso97/portugal-ai)
[![.NET App Pipeline](https://github.com/jpscardoso97/portugal-ai/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jpscardoso97/portugal-ai/actions/workflows/dotnet.yml)
[![Python API](https://github.com/jpscardoso97/portugal-ai/actions/workflows/python.yml/badge.svg)](https://github.com/jpscardoso97/portugal-ai/actions/workflows/python.yml)

## Project Description

Portugal AI is an AI project that aims to provide a useful user interface for tourists visiting Portugal. The main goal is to not only provide information about the most popular places to visit in Portugal, but to provide suggestions based on curated expertize by locals.  
Using a chatbot-like interface, the user can ask for suggestions based on their preferences, such as the type of food they like, the type of activities they enjoy, and the type of places they like to visit.  

## System Architecture

TODO

## Usage examples/screenshots

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
- Populate local vector database by running `populate-db.ipynb` notebook inside `/contextapi` folder
- Run .NET app and Python API building and running docker-compose in the repo root folder:
  ```
  docker-compose up
  ```
- Access the app at `http://localhost:5000`

## Performance/Evaluation

## CI/CD
There are three pipelines in this project:
- [.NET App Pipeline](https://github.com/jpscardoso97/portugal-ai/actions/workflows/dotnet.yml): builds and tests the .NET main application.
- [Python API Pipeline](https://github.com/jpscardoso97/portugal-ai/actions/workflows/python.yml): builds and tests the Python RAG API.
- [Docker Pipeline](): builds and deploys the necessary Docker images.

Additionally, the repository is analysed by [CodeFactor](https://www.codefactor.io/repository/github/jpscardoso97/portugal-ai) which performs static code analysis on every commit. Currently configured tools:
- Hadolint: AST-based linter for building best practice Docker images.
- YamlLint: Checks for syntax validity, weirdnesses like key repetition and cosmetic problems.
- Trivy: Spots potential security issues
- StyleLint: Helps to avoid errors and enforce consistent conventions in CSS and HTML.
- StyleCop Analyzers: Analyzes C# code to enforce a set of style and consistency rules.


