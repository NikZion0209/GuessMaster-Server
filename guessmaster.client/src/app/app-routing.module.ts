import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { GuessTheCountryComponent } from './guess-the-country/guess-the-country.component';
import { PictionaryComponent } from './pictionary/pictionary.component';
import { CathphraseComponent } from './cathphrase/cathphrase.component';
import { SpeedTextingComponent } from './speed-texting/speed-texting.component';

const routes: Routes = [
  { path: '', component: AppComponent, pathMatch: 'full' },
  { path: 'catchphrase', component: CathphraseComponent, outlet: 'game' },
  { path: 'guessTheCountry', component: GuessTheCountryComponent, outlet: 'game' },
  { path: 'pictionary', component: PictionaryComponent, outlet: 'game' },
  { path: 'speedTexting', component: SpeedTextingComponent, outlet: 'game' },
  { path: '**', redirectTo: '', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
