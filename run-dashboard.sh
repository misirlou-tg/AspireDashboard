# Dashboard will be on port 8080
# OLTP/gRPC will be on port 4317
# OLTP/HTTP (port 18890) not mapped
docker run --rm -it -p 8080:18888 -p 4317:18889 --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:8.1.0
