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

CMD [ "/bin/bash", "-c", "/opt/mssql/bin/sqlservr & sleep 20 && /usr/bin/sqlcmd -S localhost -U SA -P \"$MSSQL_SA_PASSWORD\" -i /var/opt/mssql/scripts/init.sql && tail -f /dev/null" ]
