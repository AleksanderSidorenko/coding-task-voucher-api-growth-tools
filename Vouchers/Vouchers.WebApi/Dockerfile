# Use SDK image to build solution
FROM microsoft/dotnet:2.1-sdk AS build-env

ARG RID=linux-x64

WORKDIR /build

COPY . .

RUN dotnet restore Vouchers.WebApi.csproj

RUN dotnet publish Vouchers.WebApi.csproj -r $RID -c Release -o /dist

# Build runtime image
FROM microsoft/dotnet:2.1-runtime-deps

LABEL maintainer="aleksersidorenko@gmail.com"

WORKDIR /app

RUN apt-get update && apt-get install -y curl
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/wait-for-it.sh && chmod 777 /app/wait-for-it.sh

COPY --from=build-env /dist ./

ENV ASPNETCORE_URLS http://+:5000
EXPOSE 5000

ENTRYPOINT ["./Vouchers.WebApi"]

