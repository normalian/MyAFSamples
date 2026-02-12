# C# Coding Standards & Best Practices

This document defines the mandatory rules for the code review process. The AI reviewer must check the source code against these criteria.

## 1. Naming Conventions (NC)
* **NC01 (PascalCase):** All Method names and Class names must use PascalCase (e.g., `GetUserData`, not `getUserData`).
* **NC02 (camelCase):** Local variables and private fields must use camelCase (e.g., `totalCount`, not `TotalCount`).
* **NC03 (Prefixes):** Private fields should be prefixed with an underscore (e.g., `_databaseClient`).

## 2. Security & Performance (SP)
* **SP01 (Hardcoded Secrets):** Do not hardcode API keys, connection strings, or passwords. Use environment variables or configuration providers.
* **SP02 (Async/Await):** Always use `Task.Run` or `await` for I/O bound operations. Avoid `.Result` or `.Wait()` as they can cause deadlocks.
* **SP03 (Disposal):** Objects implementing `IDisposable` (like `HttpClient` or `StreamWriter`) must be wrapped in a `using` statement or block.

## 3. Architecture & Patterns (AP)
* **AP01 (DRY - Don't Repeat Yourself):** Logic should not be duplicated across multiple methods. Extract shared logic into helper methods.
* **AP02 (Dependency Injection):** Prefer injecting dependencies through the constructor rather than instantiating them directly inside the class (Static Singleton access is an exception if justified).
* **AP03 (Testing):** Avoid making methods `public` just for the sake of unit testing. Use `[InternalsVisibleTo]` if internal methods need to be tested.

## 4. Documentation (DOC)
* **DOC01 (XML Comments):** Public methods must have XML documentation tags (`/// <summary>`) explaining the purpose and parameters.

---

## Review Output Format
For every violation found, please provide:
1.  **Rule ID:** (e.g., NC01)
2.  **Line Number:** (If identifiable)
3.  **Description:** Why it failed and how to fix it.
4.  **Code Suggestion:** A snippet of the corrected code.