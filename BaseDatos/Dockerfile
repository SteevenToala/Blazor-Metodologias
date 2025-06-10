# Usamos la imagen oficial de PostgreSQL
FROM postgres:15

# Variables de entorno para el usuario, base de datos y contraseña
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=admin123
ENV POSTGRES_DB=productdb

# Copiamos el script SQL a la carpeta de inicialización de PostgreSQL
COPY init.sql /docker-entrypoint-initdb.d/

# Exponemos el puerto por defecto
EXPOSE 5432
