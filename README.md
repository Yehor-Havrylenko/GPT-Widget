# GPT-Widget
## What is GPT-Widget?
GPT-Widget is a simple widget designed to integrate OpenAI's GPT assistant into any website. The widget helps users get information about products or services and can be customized for different tasks.

## Goal
The goal of the project is to provide an easy way to add a GPT assistant to a website using a frontend built with HTML, CSS, and JavaScript, and a backend built with ASP.NET Core.

## Setup Instructions
**1. Clone the repository**

  ``` bash
  git clone https://github.com/Yehor-Havrylenko/GPT-Widget.git
  cd GPT-Widget
  ```
**2. Modify the `.env` file**

  The `.env` file is already located in the `Backend` directory next to the `docker-compose.yml` file. Open the `.env` file and enter your OpenAI credentials:
  ``` bash
  OPENAI_ASSISTANT_ID=your_assistant_id
  OPENAI_KEY=your_openai_key
  ```
  Replace `your_assistant_id` and `your_openai_key` with your actual OpenAI credentials.

**3. Run Docker Compose**

  Run the following command to start the project:
  ``` bash
  docker-compose up
  ```
The project will be launched, and you can access the widget at http://localhost:80 
