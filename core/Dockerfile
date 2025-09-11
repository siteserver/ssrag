FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /ssrag
RUN wget https://dl.ssrag.com/rag/0.9.11/ssrag-0.9.11.tar.gz
RUN tar -xzf ssrag-0.9.11.tar.gz
RUN rm ssrag-0.9.11.tar.gz -f
RUN mv /ssrag/wwwroot /ssrag/_wwwroot
RUN echo `date +%Y-%m-%d-%H-%M-%S` > /ssrag/_wwwroot/sitefiles/version.txt

FROM base AS final

WORKDIR /app

# if you located in China, you can use aliyun mirror to speed up
RUN sed -i 's@deb.debian.org@mirrors.aliyun.com@g' /etc/apt/sources.list.d/debian.sources

# Install system dependencies
RUN apt-get update && apt-get install -y curl

COPY --from=build /ssrag .

ENTRYPOINT ["dotnet", "SSRAG.Web.dll"]
