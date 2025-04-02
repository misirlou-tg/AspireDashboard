# Dashboard (port 18888) will be on port 8080, login not required
# OLTP/gRPC (port 18889) will be on port 4317
# OLTP/HTTP (port 18890) not mapped
docker run --rm -it \
    --name aspire-dashboard \
    -p 8080:18888 \
    -p 4317:18889 \
    -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS=true \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.1.0
