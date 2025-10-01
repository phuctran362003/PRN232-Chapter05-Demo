# Demo ASP.NET Core: Routing, Model Binding & Validation

Đây là một dự án demo được xây dựng bằng ASP.NET Core nhằm minh họa ba khái niệm cốt lõi trong phát triển ứng dụng web: **Routing (Định tuyến)**, **Model Binding (Gắn kết dữ liệu)**, và **Validation (Xác thực dữ liệu)**.

Dự án bao gồm hai phần chính:
1.  **`Presentaion`**: Một ASP.NET Core Web API để quản lý dữ liệu về các bức tranh màu nước (`WatercolorsPainting`).
2.  **`RazorPage.WebApp`**: Một ứng dụng ASP.NET Core Razor Pages đóng vai trò là giao diện người dùng (UI) để tương tác với Web API.

---

This is a demo project built with ASP.NET Core to illustrate three core concepts in web application development: **Routing**, **Model Binding**, and **Validation**.

The project consists of two main parts:
1.  **`Presentaion`**: An ASP.NET Core Web API for managing data about watercolor paintings (`WatercolorsPainting`).
2.  **`RazorPage.WebApp`**: An ASP.NET Core Razor Pages application that serves as the user interface (UI) to interact with the Web API.

## Kiến trúc dự án (Project Architecture)

Dự án được cấu trúc theo kiến trúc phân lớp (Layered Architecture) để tách biệt các mối quan tâm (Separation of Concerns).

-   **`Presentaion` (Web API)**: Chịu trách nhiệm xử lý các yêu cầu HTTP, định tuyến đến các `Controller` phù hợp và trả về dữ liệu dưới dạng JSON. Đây là nơi các khái niệm **Routing**, **Model Binding**, và **Validation** được thể hiện rõ nhất ở phía backend.
-   **`RazorPage.WebApp` (UI)**: Cung cấp giao diện cho người dùng cuối. Nó gửi yêu cầu đến Web API để lấy và hiển thị dữ liệu. Trang Razor cũng thể hiện **Validation phía client (client-side)**.
-   **`Service`**: Lớp chứa logic nghiệp vụ (business logic). Nó hoạt động như một trung gian giữa lớp trình bày (API) và lớp truy cập dữ liệu.
-   **`Repository`**: Lớp chịu trách nhiệm truy cập dữ liệu, sử dụng Entity Framework Core và Repository Pattern để tương tác với cơ sở dữ liệu.

## Các khái niệm chính được minh họa (Key Concepts Illustrated)

### 1. Routing (Định tuyến)

Routing là quá trình ASP.NET Core quyết định mã nào sẽ thực thi để xử lý một yêu cầu đến dựa trên URL của nó.

-   **Trong `Presentaion` (API):**
    -   Sử dụng **Attribute Routing**. Các `Controller` (ví dụ: `WatercolorsPaintingController.cs`) được đánh dấu với `[Route("api/[controller]")]`.
    -   Các Action method được đánh dấu với các HTTP verb attribute như `[HttpGet]`, `[HttpPost]`, `[HttpGet("{id}")]` để xác định cách chúng ánh xạ tới các URL và phương thức HTTP cụ thể.
    -   Ví dụ: Một yêu cầu `GET` đến `/api/WatercolorsPainting/123` sẽ được xử lý bởi action `Get(string id)` trong `WatercolorsPaintingController`.

-   **Trong `RazorPage.WebApp`:**
    -   Sử dụng **Convention-based Routing**. Các trang Razor được định tuyến dựa trên vị trí của chúng trong thư mục `Pages`.
    -   Ví dụ: File `/Pages/Create.cshtml` sẽ tương ứng với URL `/Create`.

### 2. Model Binding (Gắn kết Model)

Model Binding tự động hóa quá trình trích xuất dữ liệu từ các phần khác nhau của yêu cầu HTTP (route, query string, request body) và chuyển đổi chúng thành các tham số cho action method.

-   **Trong `Presentaion` (API):**
    -   **`[FromRoute]`**: Trong action `Get(string id)`, tham số `id` được binding từ route template (`{id}`).
    -   **`[FromBody]`**: Trong action `Post([FromBody] CreateWatercolorsPaintingDto dto)`, toàn bộ body của yêu cầu JSON được binding vào đối tượng `CreateWatercolorsPaintingDto`.
    -   **`[FromQuery]`**: Nếu có một action nhận tham số từ query string (ví dụ: `GET /search?term=...`), `[FromQuery]` sẽ được sử dụng.

-   **Trong `RazorPage.WebApp`:**
    -   **`[BindProperty]`**: Trong các file code-behind (`.cshtml.cs`), thuộc tính model được đánh dấu `[BindProperty]` để tự động binding dữ liệu từ các trường trong form khi form được POST.

### 3. Validation (Xác thực dữ liệu)

Validation đảm bảo rằng dữ liệu người dùng gửi lên là hợp lệ trước khi được xử lý bởi ứng dụng.

-   **Server-side Validation (trong `Presentaion` API):**
    -   Sử dụng **Data Annotations** trên các lớp DTO (ví dụ: `CreateWatercolorsPaintingDto.cs`) như `[Required]`, `[StringLength]`, `[Range]`.
    -   Trong các action của Controller, `ModelState.IsValid` được kiểm tra. Nếu `false`, API sẽ tự động trả về lỗi `HTTP 400 Bad Request` với chi tiết các lỗi validation.
    -   Dự án cũng sử dụng **FluentValidation** (`WatercolorsPaintingValidator.cs`) như một cách tiếp cận nâng cao để định nghĩa các quy tắc xác thực một cách tường minh và mạnh mẽ hơn.

-   **Client-side Validation (trong `RazorPage.WebApp`):**
    -   Sử dụng các thư viện `jquery-validation` và `jquery-validation-unobtrusive`.
    -   Các **Tag Helper** như `asp-for` và `asp-validation-for` trong file `.cshtml` (ví dụ: `Create.cshtml`) tự động tạo ra các thuộc tính HTML5 `data-*` cần thiết để kích hoạt validation trên trình duyệt.
    -   Điều này cung cấp phản hồi ngay lập tức cho người dùng mà không cần gửi yêu cầu đến server.

## Hướng dẫn cài đặt và khởi chạy (Setup and Run Guide)

### Yêu cầu (Prerequisites)

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) hoặc phiên bản mới hơn.
-   Visual Studio 2022 hoặc một trình soạn thảo code khác (như VS Code).
-   SQL Server (bất kỳ phiên bản nào, ví dụ: Express, Developer).

### Các bước thực hiện (Steps)

1.  **Clone Repository:**
    ```bash
    git clone <URL_CUA_REPOSITORY_NAY>
    cd PRN231PE_SP24_123890_SE172360
    ```

2.  **Thiết lập Cơ sở dữ liệu (Database Setup):**
    -   Mở SQL Server Management Studio (SSMS) hoặc một công cụ quản lý SQL khác.
    -   Tạo một database mới. Ví dụ: `WatercolorsPainting_DB`.
    -   Thực thi các script SQL để tạo bảng và dữ liệu mẫu. Các script này có thể được tìm thấy trong dự án (thường là trong thư mục `Repository` hoặc file `prn231 scripts.txt` có thể chứa chúng).
    
3.  **Cập nhật Chuỗi kết nối (Update Connection String):**
    -   Mở file `appsettings.json` trong cả hai dự án `Presentaion` và `RazorPage.WebApp`.
    -   Tìm đến phần `ConnectionStrings` và cập nhật giá trị để trỏ đến database bạn vừa tạo.
    
    Ví dụ:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=TEN_SERVER_CUA_BAN;Database=WatercolorsPainting_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```

4.  **Khởi chạy ứng dụng (Run the Application):**

    **Cách 1: Sử dụng Visual Studio**
    -   Mở file `demo.sln` bằng Visual Studio.
    -   Trong Solution Explorer, chuột phải vào Solution và chọn `Configure Startup Projects...`.
    -   Chọn `Multiple startup projects` và đặt `Action` là `Start` cho cả hai dự án `Presentaion` và `RazorPage.WebApp`.
    -   Nhấn `F5` hoặc nút "Start" để chạy cả hai dự án cùng lúc.

    **Cách 2: Sử dụng .NET CLI**
    -   Mở hai cửa sổ terminal riêng biệt.
    -   Trong terminal thứ nhất, điều hướng đến thư mục API và chạy:
        ```bash
        cd Presentaion
        dotnet run
        ```
    -   Trong terminal thứ hai, điều hướng đến thư mục Razor Pages và chạy:
        ```bash
        cd RazorPage.WebApp
        dotnet run
        ```
    -   Mở trình duyệt và truy cập vào địa chỉ của ứng dụng Razor Page (thường là `https://localhost:7xxx` hoặc `http://localhost:5xxx`, kiểm tra output từ terminal để biết chính xác).