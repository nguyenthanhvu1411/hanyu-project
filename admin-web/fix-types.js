const fs = require('fs');
const path = require('path');

function walkSync(dir, filelist) {
  const files = fs.readdirSync(dir);
  filelist = filelist || [];
  files.forEach(f => {
    const full = path.join(dir, f);
    if (fs.statSync(full).isDirectory()) walkSync(full, filelist);
    else if (f.endsWith('.tsx') || f.endsWith('.ts')) filelist.push(full);
  });
  return filelist;
}

let fixed = 0;
const baseDir = 'c:/Luutru/WEB-ChineseLeaning/admin-web/src/features/identity';
const allFiles = walkSync(baseDir);

allFiles.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  // Replace: .id ?? 0 with .id ?? ""
  const newContent = content
    .replace(/\.id\s*\?\?\s*0/g, '.id ?? ""')
    .replace(/(?<=\bid\s*\?\?\s*)0(?!\d)/g, '""');
  if (newContent !== content) {
    fs.writeFileSync(file, newContent);
    fixed++;
    console.log('Fixed: ' + path.basename(file));
  }
});
console.log('Total: ' + fixed);
