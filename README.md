# 📊 SmartExpenseTracker

An event-driven personal finance tracker designed to automate expense logging via AI-powered OCR. This project serves as a practical laboratory for Microservices Architecture, Asynchronous Messaging, and API Gateways.

## 🏗️ Architecture & Tech Stack

The solution is built using a Microservices approach to ensure high availability and isolated scaling.

*   **Backend:** C# .NET 10 (Web API)
*   **Frontend:** React (powered by Vite)
*   **Database:** PostgreSQL (with Entity Framework Core)
*   **Message Broker:** RabbitMQ (Event-driven asynchronous communication)
*   **API Gateway:** YARP (Yet Another Reverse Proxy)
*   **AI Integration:** Google Gemini 1.5 API (Receipt OCR & Data Extraction)
*   **Infrastructure:** Docker & Docker Compose

## 🚀 Core Features

1.  **Smart Receipt Upload:** Users can upload photos of their daily receipts.
2.  **AI Data Extraction:** The isolated `OcrService` reads the image via the Gemini API and extracts a structured JSON containing: Date, Establishment, Total Value, and Category.
3.  **Asynchronous Processing:** Once extracted, the OCR service publishes a `ReceiptProcessedEvent` to RabbitMQ.
4.  **Financial Core:** The `CoreService` consumes the event from the queue and persists the categorized expense into the PostgreSQL database.
5.  **Dashboard:** A Vite/React frontend to visualize monthly expenses by category.

## 📂 Project Structure

*   `FinancialApp.Gateway/`: YARP configuration routing client requests to the appropriate internal microservice.
*   `FinancialApp.OcrService/`: Handles image uploads and interacts with the AI model.
*   `FinancialApp.CoreService/`: Manages financial business rules, database context, and repository patterns.
*   `FinancialApp.Shared/`: A shared class library containing cross-cutting concerns, DTOs, and RabbitMQ Event contracts.

## 🛠️ Getting Started

### Prerequisites
*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop)
*   [Node.js](https://nodejs.org/) (for the frontend)

### Running the Infrastructure
To spin up the PostgreSQL database and the RabbitMQ message broker locally, run:
```bash
docker-compose up -d
