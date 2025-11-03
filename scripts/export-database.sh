#!/bin/bash
# Script para exportar la base de datos actual
# Uso: ./export-database.sh [nombre-opcional]

# Configuración
CONTAINER_NAME="sismos-postgres"
DB_USER="postgres"
DB_NAME="SismosDB"
TIMESTAMP=$(date +"%Y%m%d-%H%M%S")

# Nombre del archivo
if [ -n "$1" ]; then
    FILENAME="database-$1-$TIMESTAMP.sql"
else
    FILENAME="database-snapshot-$TIMESTAMP.sql"
fi

echo "📦 Exportando base de datos..."
echo "🕒 Timestamp: $TIMESTAMP"
echo "📁 Archivo: $FILENAME"

# Verificar que el contenedor existe y está corriendo
if ! docker ps | grep -q $CONTAINER_NAME; then
    echo "❌ Error: El contenedor $CONTAINER_NAME no está corriendo"
    echo "💡 Ejecuta: docker-compose up -d"
    exit 1
fi

# Crear el dump
echo "🔄 Creando dump de la base de datos..."
if docker exec $CONTAINER_NAME pg_dump -U $DB_USER -d $DB_NAME > $FILENAME; then
    # Obtener tamaño del archivo
    SIZE=$(ls -lh $FILENAME | awk '{print $5}')
    
    echo "✅ Base de datos exportada exitosamente"
    echo "📊 Tamaño: $SIZE"
    echo "📍 Ubicación: ./$FILENAME"
    echo ""
    echo "📤 Para compartir con el equipo:"
    echo "   git add $FILENAME"
    echo "   git commit -m \"feat: snapshot de BD - $TIMESTAMP\""
    echo "   git push origin main"
    echo ""
    echo "🔄 Para restaurar:"
    echo "   docker exec -i $CONTAINER_NAME psql -U $DB_USER -d $DB_NAME < $FILENAME"
else
    echo "❌ Error creando el dump"
    rm -f $FILENAME  # Limpiar archivo vacío si falló
    exit 1
fi