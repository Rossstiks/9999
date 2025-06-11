# ProjectControl

Этот репозиторий содержит минимальный каркас приложения **ProjectControl**, предназначенного для учёта времени и управления проектами.

## Состав репозитория
- `ProjectControl.sln` – решение .NET.
- `ProjectControl.Core` – библиотека классов с основными моделями данных.
- `ProjectControl.CLI` – пример консольного приложения, демонстрирующий использование моделей.
- `ProjectControl.Data` – класс‑библиотека c `DbContext` на Entity Framework Core.
- `ProjectControl.Desktop` – WPF‑приложение с графическим интерфейсом.

## Быстрый старт
1. Требуется .NET SDK 8.0.
2. Постройте решение командой:
   ```bash
   dotnet build ProjectControl.sln
   ```
3. Запустите пример консольного приложения:
   ```bash
   dotnet run --project ProjectControl.CLI
   ```
4. Запустите WPF‑приложение:
   ```bash
   dotnet run --project ProjectControl.Desktop
   ```
> Примечание: сборка и запуск WPF-приложения возможны только на Windows с установленным workload `Microsoft.NET.Sdk.WindowsDesktop`.

WPF-приложение построено на простой модели **MVVM**. Главное окно показывает список проектов и кнопки управления таймером (старт, пауза, стоп).

Нажав кнопку `＋`, можно открыть окно создания проекта. Данные сохраняются в файл SQLite `projects.db` с помощью Entity Framework Core.

Для работы с базой данных используйте Entity Framework Core. Пример инициализации контекста находится в проекте `ProjectControl.Data`.

## Дальнейшие шаги
Подробное описание шагов по созданию полноценного приложения находится в файле `PLAN.md`.

### Использование CLI
Консольное приложение теперь демонстрирует работу `ProjectRepository` и SQLite.
При запуске будет создан файл базы `projects.db`, добавлен демо‑проект и запущен таймер на секунду.
Запустить можно командой:
```bash
dotnet run --project ProjectControl.CLI
```
