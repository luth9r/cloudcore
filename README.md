# CloudCore - Cloud Storage API

Cloud storage platform API built with ASP.NET Core, featuring secure file management, team collaboration, and intelligent storage tracking.

[College term paper]

## Installation
First install Docker. See https://www.youtube.com/watch?v=JBEUKrjbWqg
(after installing Docker, you might need to reboot your pc)
Open PowerShell and navigate to the project directory
```powershell
cd Path\to\Your\Project
```
Make sure you see docker-compose.yml in your current directory.
Then you can use those commands:
```dockerfile
# Full build and run
docker-compose up --build
# Stop all containers
docker-compose down -v
```

## Overview

CloudCore is a comprehensive backend system for cloud file storage and collaboration, similar to Google Drive or Dropbox. It provides RESTful APIs for file/folder operations, team workspaces (teamspaces), user authentication, and storage quota management.

## Key Features

### File Management
- **CRUD Operations**: Upload, download, rename, move, and delete files/folders
- **Bulk Operations**: Download multiple items as ZIP archives
- **Soft Delete**: Trash system with 30-day retention policy
- **Search**: Query items by name with pagination and sorting
- **Path Management**: Hierarchical folder structure with breadcrumb navigation

### Team Collaboration
- **Teamspaces**: Shared workspaces with separate storage pools
- **Permission System**: Granular access control (read/write/admin)
- **Member Management**: Invite users, manage roles, and track invitations
- **Collaborative Storage**: Independent storage quotas per teamspace

### Storage Management
- **Quota Tracking**: Real-time storage usage monitoring
- **Subscription Tiers**: Free, Premium, and Enterprise plans with different limits
- **Storage Calculation**: Efficient recursive folder size computation
- **Automatic Updates**: Storage usage updated on file operations

### Security
- **JWT Authentication**: Secure token-based authentication
- **Authorization Filter**: Automatic user validation on all endpoints
- **Path Traversal Protection**: Prevents unauthorized file system access
- **User Isolation**: Strict enforcement of user-owned resources


API Reference
File Operations
```
GET    /user/{userId}/mydrive    
GET    /user/{userId}/mydrive?parentId={id} 
POST   /user/{userId}/mydrive/upload  
POST   /user/{userId}/mydrive/createfolder 
PUT    /user/{userId}/mydrive/{itemId}/rename    
POST   /user/{userId}/mydrive/{itemId}/move/{target} 
DELETE /user/{userId}/mydrive/{itemId}/delete     
PUT    /user/{userId}/mydrive/{itemId}/restore   
GET    /user/{userId}/mydrive/{fileId}/download 
GET    /user/{userId}/mydrive/{folderId}/downloadfolder 
POST   /user/{userId}/mydrive/download/multiple   
GET    /user/{userId}/mydrive/trash  
GET    /user/{userId}/mydrive/folder/path/{folderId}
```
Teamspace Management
```
POST   /user/{userId}/teamspaces                          # Create teamspace
GET    /user/{userId}/teamspaces                          # List user's teamspaces
GET    /user/{userId}/teamspaces/{id}                     # Get teamspace details
PUT    /user/{userId}/teamspaces/{id}                     # Update teamspace
DELETE /user/{userId}/teamspaces/{id}                     # Delete teamspace
POST   /user/{userId}/teamspaces/{id}/members             # Add member
GET    /user/{userId}/teamspaces/{id}/members             # List members
PUT    /user/{userId}/teamspaces/{id}/members/{userId}    # Update member role
DELETE /user/{userId}/teamspaces/{id}/members/{userId}    # Remove member
POST   /user/{userId}/teamspaces/{id}/leave               # Leave teamspace
```
Teamspace Files
```
GET    /user/{userId}/teamspaces/{id}/items              # List teamspace items
POST   /user/{userId}/teamspaces/{id}/items/upload       # Upload to teamspace
POST   /user/{userId}/teamspaces/{id}/items/createfolder # Create folder
PUT    /user/{userId}/teamspaces/{id}/items/{itemId}/rename    # Rename
DELETE /user/{userId}/teamspaces/{id}/items/{itemId}/delete    # Delete
PUT    /user/{userId}/teamspaces/{id}/items/{itemId}/restore   # Restore
GET    /user/{userId}/teamspaces/{id}/items/{fileId}/download  # Download
```
Storage Information
```
GET    /user/{userId}/storage/personal                    # Personal storage info
GET    /user/{userId}/storage/teamspace/{id}              # Teamspace storage info
POST   /user/{userId}/storage/personal/recalculate        # Recalculate personal
POST   /user/{userId}/storage/teamspace/{id}/recalculate  # Recalculate teamspace
```
