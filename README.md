# PRN232 - Chapter 05 Demo: Binding, Routing, and Validation

This repository contains the source code for a demo application used in a presentation for Chapter 05 of the PRN232 course at FPT University. The demo illustrates key concepts in ASP.NET Core development.

## Core Concepts Demonstrated

This project provides practical examples of the following ASP.NET Core features:

### 1. Routing

Routing is the process of matching an incoming request to a controller action. In this project, you can see examples of:
- **Conventional Routing**: The default routing configuration in `Program.cs`.
- **Attribute-Based Routing**: Attributes like `[Route("[controller]")]`, `[HttpGet]`, `[HttpPost]`, `[HttpGet("{id}")]` are used in the controllers to define specific endpoints.

*See: `PE_PRN231_SP24_123890_SE172360_BE/Controllers/`*

### 2. Data Binding

Data binding is the process of extracting data from an incoming HTTP request and populating the parameters of a controller action method. This demo shows:
- **Binding from Route**: e.g., `[HttpGet("{id}")]` where the `id` is bound from the URL path.
- **Binding from Query String**: e.g., `[HttpGet]` with parameters that match query string keys.
- **Binding from Request Body**: e.g., `[HttpPost]` and `[HttpPut]` actions that take a model object as a parameter, which is populated from the JSON in the request body.

*See: `PE_PRN231_SP24_123890_SE172360_BE/Controllers/WatercolorsPaintingController.cs`*

### 3. Validation

Validation ensures that the data sent to the application is correct and meets the required constraints. This is demonstrated through:
- **Data Annotations**: Using attributes like `[Required]`, `[StringLength]`, etc., on model properties.
- **FluentValidation**: A more powerful and flexible way to define validation rules. A custom validator is implemented for the `WatercolorsPainting` model.

*See: `Repository/Entities/WatercolorsPainting.cs` for Data Annotations and `Service/Validators/WatercolorsPaintingValidator.cs` for FluentValidation.*

## Project Structure

The solution is organized into three main projects following a standard N-tier architecture:

- `PE_PRN231_SP24_123890_SE172360_BE`: The main ASP.NET Core Web API project containing the Controllers.
- `Service`: A class library that contains business logic and services.
- `Repository`: A class library responsible for data access, containing entities and repository classes.

## How to Run the Project

1.  **Clone the repository:**
    ```bash
    git clone <repository-url>
    ```
2.  **Open with Visual Studio:**
    - Open the `demo.sln` file with Visual Studio 2022.
    - Set `PE_PRN231_SP24_123890_SE172360_BE` as the startup project.
    - Press `F5` to run the application. The Swagger UI will open in your browser.
3.  **Run with .NET CLI:**
    - Navigate to the `PE_PRN231_SP24_123890_SE172360_BE` directory:
      ```bash
      cd PE_PRN231_SP24_123890_SE172360_BE
      ```
    - Run the project:
      ```bash
      dotnet run
      ```
    - The API will be available at `https://localhost:7088` (or a similar address shown in the console). You can access the Swagger UI at `https://localhost:7088/swagger`.
