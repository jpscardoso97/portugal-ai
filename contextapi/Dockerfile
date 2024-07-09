# Use an official Python runtime as a parent image
FROM python:3.11-slim

# Set the working directory in the container
WORKDIR /app

# Copy the requirements file into the container at /app
COPY requirements.txt /app/requirements.txt

# Install any needed packages specified in requirements.txt
RUN pip install --no-cache-dir -r /app/requirements.txt

# Copy the current directory contents into the container at /app
COPY ./app /app

COPY bootstrap_db.py /app

# Run the database bootstrap script
RUN python bootstrap_db.py

EXPOSE 5002

# Run uvicorn server on container startup
CMD ["fastapi", "run", "main.py", "--port", "5002"]
