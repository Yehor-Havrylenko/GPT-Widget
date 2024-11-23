FROM nginx:alpine

COPY ./Deployment/Docker/Frontend/nginx.conf /etc/nginx/nginx.conf
COPY ./Frontend/ /usr/share/nginx/html

RUN rm -rf /var/cache/apk/* /tmp/* /var/tmp/*

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]