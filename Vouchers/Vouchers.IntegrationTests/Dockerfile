# Use SDK image to build solution
FROM microsoft/dotnet:2.1-sdk AS build-env

ARG RID=linux-x64

WORKDIR /build

COPY Vouchers.WebApi/Vouchers.WebApi.csproj Vouchers.WebApi/
COPY Vouchers.IntegrationTests/Vouchers.IntegrationTests.csproj Vouchers.IntegrationTests/

RUN dotnet restore Vouchers.IntegrationTests/Vouchers.IntegrationTests.csproj

COPY Vouchers.WebApi Vouchers.WebApi
COPY Vouchers.IntegrationTests Vouchers.IntegrationTests

RUN dotnet publish Vouchers.IntegrationTests/Vouchers.IntegrationTests.csproj -r $RID -c Release -o /dist

# Build runtime image
FROM microsoft/dotnet:2.1-sdk

LABEL maintainer="aleksersidorenko@gmail.com"

WORKDIR /app

RUN apt-get update && apt-get install -y curl
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/wait-for-it.sh && chmod 777 /app/wait-for-it.sh

COPY --from=build-env /dist ./

ENTRYPOINT ["dotnet", "vstest", "./Vouchers.IntegrationTests.dll"]
