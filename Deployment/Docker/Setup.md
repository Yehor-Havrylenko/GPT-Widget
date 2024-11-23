# Setup Instructions
### 1. Modify the `.env` file

The `.env` file is already located in the `Backend` directory next to the `docker-compose.yml` file. Open the `.env` file and enter your OpenAI credentials:
  ``` bash
  OPENAI_ASSISTANT_ID=your_assistant_id
  OPENAI_KEY=your_openai_key
  ```
Replace `your_assistant_id` and `your_openai_key` with your actual OpenAI credentials.

### 3. Run Docker Compose

Run the following command to start the project:
  ``` bash
  docker-compose up
  ```
The project will be launched, and you can access the widget at http://localhost:80 
