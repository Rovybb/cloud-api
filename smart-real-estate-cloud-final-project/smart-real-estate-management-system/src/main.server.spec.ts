import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { config } from './app/app.config.server';

describe('Bootstrap Application', () => {
  it('should call bootstrapApplication with AppComponent and config', async () => {
    // Spionăm funcția bootstrapApplication
    const bootstrapSpy = spyOn<any>(globalThis, 'bootstrapApplication').and.returnValue(Promise.resolve());

    // Importăm și executăm bootstrap
    const bootstrap = await import('./main'); // Asumăm că fișierul tău este numit `main.ts`

    // Verificăm că bootstrapApplication a fost apelată corect
    expect(bootstrapSpy).toHaveBeenCalledWith(AppComponent, config);
  });

  it('should export a default function', async () => {
    const bootstrap = await import('./main');
  });
});
