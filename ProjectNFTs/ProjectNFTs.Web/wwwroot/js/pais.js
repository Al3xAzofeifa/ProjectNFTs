function getPaisById(idPais, IdDiv) {
    const myRequest = "/pais/GetPaisById?id=" + idPais
    fetch(myRequest)
        .then((response) => response.json())
        .then((data) => {
            const div = document.getElementById(IdDiv);
            console.log(data)
            // Beware! Properties in CamelCase!
            div.innerHTML = data.descripcion
        });
}
