docker build -t vouchers.integration-tests:latest -f .\Vouchers.IntegrationTests\Dockerfile .
cd .\Vouchers.WebApi
docker build -t vouchers.webapi:latest .
docker-compose up --abort-on-container-exit