If you want to deploy a new react or angular project in the storage account static webite, please perform following steps
a) Upload the new angular or react prject as a new folder in project path: SPA_TUTORIALS/REACT_SAMPLES/**<Name_Of_New_React_Or_Angular_Project>**

b) Change the project path in "React-StorageAccount-CI" Azure DevOps pipeline to following =>
   ProjectPath: 'SPA_TUTORIALS/REACT_SAMPLES/**<Name_Of_New_React_Or_Angular_Project>**' 
   e.g. ProjectPath: 'SPA_TUTORIALS/REACT_SAMPLES/REACT-ECOMMERCE'

c) For Angular projects, change the project path in "**Angular-StorageAccount-CI**"
