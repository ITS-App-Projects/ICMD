import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AppConfig } from "src/app/app.config";

@Component({
  selector: "app-error1",
  templateUrl: "./error1.component.html",
})
export class Error1Component implements OnInit {

  constructor(private router: Router, private appConfig: AppConfig) { }

  ngOnInit(): void { }

  protected sendToLogIn() {
    this.appConfig.removeUserAndSendToLogin(this.router, false);
  }
}
