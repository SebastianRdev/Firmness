#!/bin/bash

# Colores
GREEN='\033[0;32m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

echo -e "${CYAN}🚀 Iniciando entorno Firmness...${NC}"

# Levantar contenedores
docker compose up --build -d

echo ""
echo -e "${GREEN}✅  ¡Entorno levantado exitosamente!${NC}"
echo ""
echo -e "   📱 ${CYAN}Customer App:${NC}   http://localhost:8083"
echo -e "   💻 ${CYAN}WebAdmin:${NC}       http://localhost:8082"
echo -e "   🔌 ${CYAN}API Swagger:${NC}    http://localhost:8081/index.html"
echo ""
