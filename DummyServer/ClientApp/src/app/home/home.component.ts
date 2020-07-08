import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subject } from "rxjs";
import { switchMap, startWith } from "rxjs/operators";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public builds: Build[];
  public branches: Branch[];
  public definitions: Definition[];
  public users: User[];
  private http: HttpClient;
  private baseUrl: string;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;

    this.refreshBuilds.asObservable().pipe(
      startWith(undefined),
      switchMap(() => http.get<Build[]>(baseUrl + 'build'))
    ).subscribe(result => {
        this.builds = result;
      },
      error => console.error(error));

    this.refreshBranches.asObservable().pipe(
      startWith(undefined),
      switchMap(() => http.get<Branch[]>(baseUrl + 'branch'))
    ).subscribe(result => {
        this.branches = result;
      },
      error => console.error(error));

    this.refreshDefinitions.asObservable().pipe(
      startWith(undefined),
      switchMap(() => http.get<Definition[]>(baseUrl + 'definition'))
    ).subscribe(result => {
        this.definitions = result;
      },
      error => console.error(error));

    this.refreshUser.asObservable().pipe(
      startWith(undefined),
      switchMap(() => http.get<User[]>(baseUrl + 'user'))
    ).subscribe(result => {
        this.users = result;
      },
      error => console.error(error));
  }

  private refreshBuilds = new Subject();
  private refreshBranches = new Subject();
  private refreshDefinitions = new Subject();
  private refreshUser = new Subject();

  public refresh() {
    this.refreshBuilds.next();
  }

  public newBranch(name: string) {
    var headers = new HttpHeaders().set('Content-Type', 'application/json');
    this.http.post<Branch>(this.baseUrl + 'branch', JSON.stringify(name), { headers: headers })
      .subscribe(r => this.refreshBranches.next());

  }

  public deleteBranch(name: string) {
    this.http.delete<Branch>(this.baseUrl + 'branch?name=' + name).subscribe(r => this.refreshBranches.next());
  }

  public newDefinition(name: string) {
    var headers = new HttpHeaders().set('Content-Type', 'application/json');
    this.http.post<Branch>(this.baseUrl + 'definition', JSON.stringify(name), { headers: headers })
      .subscribe(r => this.refreshDefinitions.next());
  }

  public deleteDefinition(name: string) {
    this.http.delete<Branch>(this.baseUrl + 'definition?name=' + name).subscribe(r => this.refreshDefinitions.next());
  }

  public newUser(name: string) {
    var headers = new HttpHeaders().set('Content-Type', 'application/json');
    this.http.post<Branch>(this.baseUrl + 'user', JSON.stringify(name), { headers: headers })
      .subscribe(r => this.refreshUser.next());
  }

  public deleteUser(name: string) {
    this.http.delete<Branch>(this.baseUrl + 'user?name=' + name).subscribe(r => this.refreshUser.next());
  }

}

interface Build {
  branchName: string;
  id: string;
}

interface Branch {
  fullName: string;
}

interface Definition {
  name: string;
}

interface User {
  uniqueName: string;
}
