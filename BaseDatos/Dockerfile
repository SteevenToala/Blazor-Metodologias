FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y \
    MSSQL_SA_PASSWORD=YourStrong!Passw0rd \
    MSSQL_PID=Developer

USER root

RUN apt-get update && apt-get install -y curl apt-transport-https gnupg2 && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools unixodbc-dev && \
    ln -sfn /opt/mssql-tools/bin/sqlcmd /usr/bin/sqlcmd && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

USER mssql

RUN mkdir -p /var/opt/mssql/scripts
WORKDIR /var/opt/mssql/scripts
COPY init.sql .

EXPOSE 1433

CMD /bin/bash -c "\
/opt/mssql/bin/sqlservr & \
echo 'Esperando a que SQL Server se inicie...' && \
for i in {1..60}; do \
  /usr/bin/sqlcmd -S localhost -U SA -P \"$MSSQL_SA_PASSWORD\" -Q 'SELECT 1' > /dev/null 2>&1 && break; \
  echo \"Intento $i de 60: Esperando SQL Server...\"; \
  sleep 3; \
done && \
echo 'SQL Server iniciado. Ejecutando script de inicialización...' && \
/usr/bin/sqlcmd -S localhost -U SA -P \"$MSSQL_SA_PASSWORD\" -i /var/opt/mssql/scripts/init.sql -o /var/opt/mssql/scripts/init.log && \
echo 'Script ejecutado exitosamente. Ver log en /var/opt/mssql/scripts/init.log' && \
tail -f /dev/null"
