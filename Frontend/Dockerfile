FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

FROM node:20-alpine AS final
WORKDIR /app

COPY --from=build /app/.output ./.output

ENV NODE_ENV=production
ENV PORT=3000
ENV NUXT_PUBLIC_API_BASE=http://backend:8080

EXPOSE 3000

CMD ["node", ".output/server/index.mjs"]