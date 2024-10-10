# Standard Naming Convention
- Use `kebab-case` to jobs name
- Use `snake_case` to variabl names or inputs

# Adding new service
- Go to `build-and-publish.yml`
- Update the `build-and-publish` job to add the new microservice to build
```yml
  name: Publish
    dotnet publish <service-project> -c release -o $(pwd)/out/<service-name>
```
- Once the above has been setup
- Create it's own deployment and copy one of the `deploy-dev-<name>` stuff