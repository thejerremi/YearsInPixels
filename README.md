# Year in Pixels

## Description
Year in Pixels is a web application designed to monitor mood and mental well-being throughout the year. It allows users to record their daily emotional state, a brief summary of the day, and a detailed journal entry. The primary goal of the project is to visualize an emotional map of the year, helping users identify mood patterns and engage in self-reflection.

### Key Features

* Secure sign-in so your mood journal stays private to your account.
* Year-in-pixels style mood map - interactive grid for the whole year, with per-day add and edit.
* Year navigation to browse and load entries for past years.
* Summary metrics - year completion, year progress, and average mood.
* Today entry - quick access to log or update today’s mood when viewing the current year.
* Flashback system - pick a random past entry.
* Multilingual interface with an in-app language switcher.
* Responsive UI built with Vuetify for a consistent experience on desktop and mobile.

## Technical Details

### Tech Stack
* Backend: .NET 9, Entity Framework Core, SQLite.
* Frontend: Nuxt4, Vue.js, Vuetify, Typescript.

## Local Setup Instructions

### Prerequisites
* .NET SDK (version 9.0 or newer)
* Node.js and npm (or alternative)

### Clone repo
  ```bash
  git clone https://github.com/thejerremi/YearsInPixels
  ```

### Backend
1. Navigate to the WebAPI project folder:
   ```bash
   cd Backend/YearsInPixels.WebAPI
   ```
2. Initialize User Secrets and set the JWT Key:
   ```bash
   dotnet user-secrets init

   # Note: For HMAC-SHA512, the key must be at least 64 characters long
   dotnet user-secrets set "JwtSettings:Key" "your_very_long_and_secure_secret_key_containing_at_least_64_characters"
   ```
3. Run the API server:
   ```bash
   dotnet run
   ```

### Frontend
1. Navigate to the frontend folder:
   ```bash
   cd Frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the application:
   ```bash
   npm run dev
   ```

## AI-Assisted Development

This project was developed with the support of advanced Artificial Intelligence tools to optimize the architecture and streamline the development workflow:

* **Gemini (Large Language Model)**: Utilized as a technical consultant for database schema design and backend architectural planning.
* **Cursor (AI Code Editor)**: Leveraged for the frontend implementation phase. It played a significant role in accelerating the development of UI components and refining the user experience (UX). Cursor rules set is available [here](https://github.com/thejerremi/YearsInPixels/tree/main/Frontend/.cursor/rules).
