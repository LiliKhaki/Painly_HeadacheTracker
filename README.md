![Banner](Screenshots/banner.png)

# HeadacheTracker

HeadacheTracker is a .NET MAUI application for tracking headache episodes. The app allows users to:

- Keep a calendar of headache entries

- Add, edit, and delete headache records

- Track medications associated with each episode

- View monthly and yearly statistics

## Disclaimer
This app is not a medical device and is not intended to provide medical advice, diagnosis, or treatment.  
It is a personal tracking tool to help users monitor headache episodes and medications.  
Users should consult a healthcare professional for medical concerns.

## Features
#### Calendar

- Displays days of the month

- Highlights today’s date

- Highlights days with headache entries

- Allows selecting a day to view entries

#### Headache Entries

- Add, edit, and delete headache episodes

- Associate medications with each episode

#### Statistics

- Number of days with pain and pain-free days

- Total headache episodes

- Average pain intensity

- Days with medication usage

- Longest streak of consecutive pain-free days (yearly)

## Architecture

MVVM (Model-View-ViewModel) pattern

#### Application Layers:

- Domain / Application – business logic, use-case GetHeadachesByMonth

- Repositories – IHeadacheRepository, IMedicationRepository

- UI – pages using CollectionView (7 columns for calendar)

- Database: SQLite via SQLiteAsyncConnection

#### Localization

Supports multiple languages via resource files:

- AppResources.resx (default)

- AppResources.de.resx (German)

- AppResources.ru.resx (Russian)

UI automatically adapts to the device’s current culture

## Installation & Running

Clone the repository:

1. git clone https://github.com/USERNAME/HeadacheTracker.git

2. Open the project in Visual Studio 2022/2023 with MAUI support

3. Set up an Android emulator or a Windows target

4. Run the project in Debug/Run mode

5. The app automatically creates a local SQLite database

#### Dependencies:

.NET 7 or higher

.NET MAUI

SQLite-net-pcl

## Project Structure
HeadacheTracker.Maui/

├─ Models/          # Data models (HeadacheEntry, MedicationEntry, StatisticsSummary)

├─ ViewModels/      # ViewModels for pages and logic

├─ Views/           # XAML pages

├─ Services/        # Business logic (StatisticsService)

├─ Repositories/    # Database access

## Future Plans

- Complete the statistics page with charts and graphs

- Enhance calendar UI (highlighting entries, interactive selection)

- Improve Android performance

- iOS deployment

License

This project is licensed under the MIT License.
