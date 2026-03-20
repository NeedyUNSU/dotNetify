# How to start app 
## First Backend
- Init
```Powershell
cd backend
dotnet restore
dotnet run
```
- If data migration was not done, before init do:
```shell
dotnet ef migrations remove 
dotnet ef migrations add InitDB
dotnet ef database update
```
## Next Frontend
- First startup:
```shell
cd frontend
echo "NEXT_PUBLIC_API_URL=http://localhost:5020" >> .env.local
echo "TOKEN_COOKIE_NAME=.AspNetCore.Identity.Application" >> .env.local
echo "NODE_TLS_REJECT_UNAUTHORIZED=0" >> .env.local
pnpm install
pnpm dev
```
- Default Startup:
```shell
cd frontend
$env:NEXT_PUBLIC_API_URL="http://localhost:5020"
$env:TOKEN_COOKIE_NAME=".AspNetCore.Identity.Application"
$env:NODE_TLS_REJECT_UNAUTHORIZED="0"
pnpm install
pnpm dev
```
## Run Tests
```shell
cd Tests
dotnet test
```

# .NETify - Fullstack Spotify Clone By NacomiTagiera fixes by NeedyUNSU

A modern, fullstack music streaming application built with .NET 8 and Next.js 15, featuring a comprehensive music library, playlist management, and real-time audio playback.

## 🎵 Project Overview

.NETify is a Spotify-inspired music streaming platform that provides users with a complete music experience including browsing songs by genre, creating playlists, following artists, and tracking listening history with detailed statistics.

## 🚀 Key Features

### Frontend Features
- **Modern UI/UX**: Spotify-inspired dark theme with responsive design
- **Real-time Audio Player**: Full-featured music player with play/pause, skip, volume control, and progress tracking
- **Music Library**: Browse songs by genre with grid and table views
- **Playlist Management**: Create, edit, and manage custom playlists
- **Artist Following**: Follow favorite artists and view their songs
- **Listening History**: Track and view detailed listening statistics
- **Search Functionality**: Search for songs, artists, and playlists
- **User Authentication**: Secure login and registration system
- **Admin Panel**: Administrative interface for content management

### Backend Features
- **RESTful API**: Comprehensive API endpoints for all music operations
- **Artist Management**: CRUD operations for artist profiles and songs
- **Song Management**: Upload, categorize, and manage music tracks
- **User Statistics**: Track listening time, favorite genres, and play counts
- **Friendship System**: Connect with other users
- **Database Management**: PostgreSQL with Entity Framework Core

## 🛠️ Technology Stack

### Frontend Technologies
- **Framework**: Next.js 15 with App Router
- **Language**: TypeScript
- **Styling**: Tailwind CSS with custom animations
- **UI Components**: 
  - Radix UI primitives (Dialog, Dropdown, Slider, etc.)
  - Custom component library with shadcn/ui
- **State Management**: Zustand for global state
- **Data Fetching**: React Server Components with async/await
- **Tables**: TanStack Table for data display
- **Icons**: Lucide React icons
- **Image Optimization**: Next.js Image component with Plaiceholder for blur effects
- **Development Tools**:
  - ESLint with TypeScript support
  - Prettier for code formatting
  - pnpm for package management

### Backend Technologies
- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Database**: PostgreSQL with Npgsql provider
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **API Documentation**: Swagger/OpenAPI
- **Object Mapping**: AutoMapper
- **Architecture**: Clean Architecture with separate layers

### Development Tools
- **Package Manager**: pnpm (frontend), NuGet (backend)
- **Code Quality**: ESLint, Prettier, TypeScript strict mode
- **Database Migrations**: EF Core Migrations
- **IDE Support**: Visual Studio Code, Visual Studio

## 📁 Project Structure

```
dotNetify/
├── frontend/                 # Next.js application
│   ├── src/
│   │   ├── app/             # App Router pages
│   │   ├── components/      # Reusable UI components
│   │   ├── features/        # Feature-based modules
│   │   ├── hooks/           # Custom React hooks
│   │   ├── lib/             # Utility functions
│   │   ├── store/           # Zustand stores
│   │   └── types/           # TypeScript type definitions
│   ├── public/              # Static assets
│   └── package.json         # Frontend dependencies
├── backend/                 # .NET Core API
│   ├── Controllers/         # API controllers
│   ├── Models/              # Entity models
│   ├── ModelsDTO/           # Data transfer objects
│   ├── Services/            # Business logic
│   ├── Data/                # Database context
│   ├── Migrations/          # EF Core migrations
│   └── dotNETify.csproj     # Backend dependencies
└── README.md
```

## 🎯 Core Functionality

### Music Player
- Real-time audio streaming with HTML5 Audio API
- Play/pause, skip forward/backward controls
- Volume control and progress seeking
- Queue management and playlist playback
- Current song display with album artwork

### User Experience
- Responsive design for desktop and mobile
- Dark theme optimized for music listening
- Smooth animations and transitions
- Intuitive navigation with sidebar and header
- Toast notifications for user feedback

### Data Management
- Efficient data fetching with React Server Components
- Optimistic UI updates for better user experience
- Image optimization with blur placeholders
- Pagination and infinite scrolling support

## 🔧 Development Contributions

### Frontend Implementation (Full Responsibility)
- Complete Next.js application architecture
- All UI components and layouts
- State management with Zustand
- Audio player implementation
- Responsive design and animations
- TypeScript type safety throughout

### Backend Contributions
- **Artist Management API**: Complete CRUD operations for artist profiles
- **Song Management API**: Endpoints for song upload, categorization, and retrieval
- **Database Design**: Entity relationships for artists and songs
- **API Integration**: Seamless frontend-backend communication

## 🚀 Getting Started

### Prerequisites
- Node.js 18+ and pnpm
- .NET 8 SDK
- PostgreSQL database

### Frontend Setup
```bash
cd frontend
pnpm install
pnpm dev
```

### Backend Setup
```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

## 📊 Performance Features
- Image optimization with Next.js Image component
- Lazy loading for better performance
- Efficient state management with Zustand
- Server-side rendering for improved SEO
- Optimized bundle size with tree shaking

## 🎨 Design System
- Consistent color palette with brand colors
- Typography scale with proper hierarchy
- Spacing system based on Tailwind CSS
- Component variants with class-variance-authority
- Accessible UI components with proper ARIA labels
