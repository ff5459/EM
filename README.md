# EM

Тестовое задание для Effective Mobile, выполненое по техническому заданию:
https://drive.google.com/file/d/1vmkNvGdWtarYZqIa9zFR9UGIih6waef-/view?pli=1

## Требования

Перед началом убедитесь, что у вас установлены следующие инструменты:

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Установка

Клонируйте репозиторий:
git clone https://github.com/ff5459/EM.git

## Запуск

прошишите в командной строке
docker compose up -d --build

Сервис будет доступен по адресу http://localhost:80

## Исползование

1. Загрузить данные (тестовые находятся в EM/data.csv)
2. Нажать Upload
3. Выбрать регион
4. Опционально скачать результат

## Остановка

docker compose down
