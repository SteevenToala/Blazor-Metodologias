window.downloadFile = (filename, contentType, content) => {
    // Crear un blob con el contenido
    const blob = new Blob([content], { type: contentType });
    
    // Crear una URL temporal para el blob
    const url = window.URL.createObjectURL(blob);
    
    // Crear un elemento anchor temporal
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = filename;
    
    // Agregar al DOM, hacer clic y remover
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);
    
    // Limpiar la URL temporal
    window.URL.revokeObjectURL(url);
};
