FROM dunglas/frankenphp:php8.4

WORKDIR /app

RUN install-php-extensions pdo_mysql mysqli

COPY . /app

EXPOSE 8080

CMD ["sh", "-c", "frankenphp php-server -r /app/public -l 0.0.0.0:${PORT:-8080}"]