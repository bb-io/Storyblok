# Blackbird.io Storyblok  
  
Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.  
  
## Introduction  
  
<!-- begin docs -->  
  
Storyblok is a headless content management. This Storyblok application primarily centers around story and task management.  

## Connecting  
  
1. Navigate to apps and search for Storyblok. If you cannot find Notion then click _Add App_ in the top right corner, select Storyblok and add the app to your Blackbird environment.  
2. Click _Add Connection_.  
3. Name your connection for future reference e.g. 'My client'.  
4. Go to your Storyblok profile and select `Personal access tokens` section
5. Generate a new API token
7. Copy generated token and paste it to the appropriate field in Blackbird
8. Click _Connect_.  
9. Confirm that the connection has appeared and the status is _Connected_.  
  
## Actions  
 
### Components

- **Search/Get/Create/Delete components**

### Releases

- **Search/Get/Create/Delete release**

### Spaces

- **Search/Get/Create/Delete/Backup spaces**

### Stories

- **Export story content** returns story content as an HTML file.
- **Import story content** updates story content from an HTML file.
- **Search/Get/Create/Update/Delete stories**
- **Publish/Unpublish story**
- **Get story as HTML for translation** gets story as an HTML file for field level localization.
- **Translate story from HTML file** updates story from an HTML file using field level localization.

### Tasks

- **Search/Get/Create/Delete tasks**

## Events

- **On story published**
- **On story unpublished**
- **On story moved**
- **On story deleted**
- **On asset created**
- **On asset deleted**
- **On asset replaced**
- **On asset restored**
- **On user added**
- **On user removed**
- **On user roles updated**
- **On data source entry saved**
- **On pipeline deployed**
- **On release merged**
- **On workflow changed**
  
## Feedback  
  
Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.
  
<!-- end docs -->
