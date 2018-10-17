
document.querySelector("#ticket-sbmt").addEventListener("click", (e) => {

				var form = $("#form-valid").validate();
				form.form();
				if (form.valid()) {

								let formsArray = document.querySelectorAll(".file-form");
								for (let i = 0; i < formsArray.length; i++) {
												let childArray = formsArray[i].querySelectorAll("input");
												for (let j = 0; j < childArray.length; j++) {
																childArray[j].name = `ticketAttachments[${i}].` + childArray[j].name;
												};
								};
				}

});

document.querySelector("#attch-btn").addEventListener("click", (e) => {
				let field = document.querySelector("#attch-field");
				let newField = document.createElement("fieldset");
				newField.id = "my-fieldset";
				newField.innerHTML = `<legend>Attachment</legend>
												<div class="form-group file-form" >
																<label class="control-label col-md-2" for="FileBase">Choose a file</label>
																<div class="col-md-10">
																				<input class="ticket-input form-control" type="file" name="FileBase" required />
																</div>
																<label class="control-label col-md-2" for="FileBase">File description</label>
																<div class="col-md-10">
																				<input class="ticket-input form-control" type="text" name="FileDescription" required />
																</div>
																<div class="col-md-offset-2 col-md-10">
																				<a class="btn btn-danger" id="remove-btn">REMOVE</a>
																</div>
                            </div >`;
				field.appendChild(newField);
});

document.querySelector("#attch-field").addEventListener("click", (e) => {
				if (e.target.id == "remove-btn") {
								let fieldSet = e.target.closest("#my-fieldset");
								fieldSet.remove();
				}

});
