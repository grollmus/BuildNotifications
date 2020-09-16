import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subject, Observable } from "rxjs";
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

  public selectedDefinition: Definition;
  public selectedBranch: Branch;
  public selectedUser: User;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {


  }


  ngOnInit() {
    this.listenToUpdate<Build[]>(this.refreshBuilds, 'build', r => { this.builds = r; });
    this.listenToUpdate<Branch[]>(this.refreshBranches,
      'branch',
      r => {
        this.branches = r;
        this.selectedBranch = this.selectedValueOrDefault(this.selectedBranch, r);
      });
    this.listenToUpdate<Definition[]>(this.refreshDefinitions,
      'definition',
      r => {
        this.definitions = r;
        this.selectedDefinition = this.selectedValueOrDefault(this.selectedDefinition, r);
      });
    this.listenToUpdate<User[]>(this.refreshUser,
      'user',
      r => {
        this.users = r;
        this.selectedUser = this.selectedValueOrDefault(this.selectedUser, r);
      });
  }

  private listenToUpdate<T>(subject: Subject<Object>, path: string, resultHandler: (result: T) => any) {
    subject.pipe(
      startWith(undefined),
      switchMap(() => this.http.get<T>(this.baseUrl + path))
    ).subscribe(resultHandler, error => console.error(error));
  }

  private selectedValueOrDefault<T>(currentValue: T, possibleValues: Array<T>): T {
    if (!currentValue || possibleValues.indexOf(currentValue) < 0)
      return this.firstOrDefault(possibleValues);
  }

  private firstOrDefault<T>(array: Array<T>) {
    return array.length === 0 ? null : array[0];
  }

  private refreshBuilds = new Subject();
  private refreshBranches = new Subject();
  private refreshDefinitions = new Subject();
  private refreshUser = new Subject();

  public refresh() {
    this.refreshBuilds.next();
  }

  public newBranch(name: string) {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    this.http.post<Branch>(this.baseUrl + 'branch', JSON.stringify(name), { headers: headers })
      .subscribe(r => this.refreshBranches.next());

  }

  public deleteBranch(name: string) {
    this.http.delete<Branch>(this.baseUrl + 'branch?name=' + name).subscribe(r => this.refreshBranches.next());
  }

  public newDefinition(name: string) {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    this.http.post<Branch>(this.baseUrl + 'definition', JSON.stringify(name), { headers: headers })
      .subscribe(r => this.refreshDefinitions.next());
  }

  public deleteDefinition(name: string) {
    this.http.delete<Branch>(this.baseUrl + 'definition?name=' + name).subscribe(r => this.refreshDefinitions.next());
  }

  public newUser(name: string) {
    const headers = new HttpHeaders().set('Content-Type', 'application/json');
    this.http.post<Branch>(this.baseUrl + 'user', JSON.stringify(name), { headers: headers })
      .subscribe(r => this.refreshUser.next());
  }

  public deleteUser(name: string) {
    this.http.delete<Branch>(this.baseUrl + 'user?name=' + name).subscribe(r => this.refreshUser.next());
  }

  public addBuild() {
    if (!this.selectedBranch || !this.selectedDefinition || !this.selectedUser)
      return;

    const build = new Build();
    build.branchName = this.selectedBranch.fullName;
    build.userName = this.selectedUser.uniqueName;
    build.definitionName = this.selectedDefinition.name;

    this.postBuild(build);
  }

  private postBuild(build: Build) {
    this.http.post<Build>(this.baseUrl + 'build', build)
      .subscribe(r => this.refreshBuilds.next());
  }

  public updateBuildStatus(build: Build, status: number) {
    build.status = status;
    this.postBuild(build);
  }

  public updateBuildReason(build: Build, reason: number) {
    build.reason = reason;
    this.postBuild(build);
  }

  public permutate() {
    this.http.post(this.baseUrl + 'build/permutate', null).subscribe(r => this.refreshBuilds.next());
  }

  public randomizeBuildStatus() {
    this.http.post(this.baseUrl + 'build/randomizeBuildStatus', null).subscribe(r => this.refreshBuilds.next());
  }

  public deleteBuild(build: Build) {
    this.http.delete<Build>(this.baseUrl + 'build?id=' + build.id).subscribe(r => this.refreshBuilds.next());
  }
}

class Build {
  branchName: string;
  definitionName: string;
  userName: string;
  id: string;
  status: number;
  reason: number;
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
