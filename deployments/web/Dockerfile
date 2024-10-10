# Note: The test and build will run on Jenkins Slave

# Run the application on nginx container
FROM nginx:latest
COPY icmd-main/Frontend/dist /usr/share/nginx/html
COPY deployments/web/nginx.conf /etc/nginx/nginx.conf

WORKDIR /usr/share/nginx/html
