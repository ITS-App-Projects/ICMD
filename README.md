# Installation or upgrade of source code
- Go to this repository <https://gitlab.com/setavinod/icmd/-/tree/main
- Download the file as zip
- Extract the file and replace the one in `icmd-main`
  - Replace only the source code and make sure on the angular side should not replace the `environments`
- Once done. Run the jenkins pipeline for upgrade
```typescript
  // Files need to save
  Backend/Program.cs

  Frontend/yarn.lock
  Frontend/package.json
  Frontend/angular.json
  Frontent/environments/environment.dev.ts
  Frontent/environments/environment.prod.ts
```

# Deployment process
- The `deployment` folder is created by us.
- Make changes on the version if needed.
- Go on for the checking