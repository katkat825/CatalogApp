var checkbox = document.getElementById("ChangeTheme"); 

if (sessionStorage.getItem("mode") == "dark") {
    darkMode(); 
  } else {
    lightMode();
  }

checkbox.addEventListener("change", function() {
if (checkbox.checked) {
  darkMode();
} else {
  lightMode(); 
}
});

function darkMode() {
  document.body.classList.add("dark-mode");
  checkbox.checked = true;
  sessionStorage.setItem("mode", "dark");
}

function lightMode() {
  document.body.classList.remove("dark-mode");
  checkbox.checked = false;
  sessionStorage.setItem("mode", "light");
}
