import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { FlagWhizComponent } from './flag-whiz/flag-whiz.component';
import { WordSnapComponent } from './word-snap/word-snap.component';
import { WordyWondersComponent } from './wordy-wonders/wordy-wonders.component';
import { TextBlitzComponent } from './text-blitz/text-blitz.component';

const routes: Routes = [
  { path: '', component: AppComponent, pathMatch: 'full' },
  { path: 'wordyWonders', component: WordyWondersComponent, outlet: 'game' },
  { path: 'flagWhiz', component: FlagWhizComponent, outlet: 'game' },
  { path: 'wordSnap', component: WordSnapComponent, outlet: 'game' },
  { path: 'textBlitz', component: TextBlitzComponent, outlet: 'game' },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
