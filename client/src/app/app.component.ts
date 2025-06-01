import { ButtonModule } from 'primeng/button';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ToggleButtonModule, ButtonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})  
export class AppComponent {
  title = 'hrms-web-app';

  toggleDarkMode() {
    const element = document.querySelector('html');
    if (element) {
      element.classList.toggle('my-app-dark');
    }
  }
}
