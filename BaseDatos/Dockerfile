# Usamos la imagen oficial de Microsoft SQL Server para Linux
FROM mcr.microsoft.com/mssql/server:2022-latest

# Establecemos las variables de entorno requeridas por SQL Server
ENV ACCEPT_EULA=Y \
    MSSQL_SA_PASSWORD=YourStrong!Passw0rd \
    MSSQL_PID=Developer

# Creamos un directorio para los scripts de inicialización
RUN mkdir -p /var/opt/mssql/scripts
WORKDIR /var/opt/mssql/scripts

# Copiamos los scripts a la imagen
COPY init.sql .

# Exponemos el puerto por defecto de SQL Server
EXPOSE 1433

# Comando de inicio personalizado para ejecutar SQL Server y luego el script
CMD [ "/bin/bash", "-c", "/opt/mssql/bin/sqlservr & \
    sleep 20 && \
    /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P \"$MSSQL_SA_PASSWORD\" -i /var/opt/mssql/scripts/init.sql && \
    tail -f /dev/null" ]
