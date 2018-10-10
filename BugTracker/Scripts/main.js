document.querySelector("#srch-btn").addEventListener("click", (e) => {

				let selecValue = document.querySelector("#lunch").value;
				let searchQuery = document.querySelector("#search-bar").value;
				window.location.href = `${selecValue}?query=${searchQuery}`;

});