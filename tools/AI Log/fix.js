const fs = require('fs');
const path = require('path');

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach(file => {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) { 
      results = results.concat(walk(file));
    } else if (file.endsWith('.tsx')) { 
      results.push(file);
    }
  });
  return results;
}

const files = walk('./src/components');
files.push('./src/app/page.tsx');
files.push('./src/app/project/[id]/workspace/page.tsx');
files.push('./src/app/project/[id]/workspace/step1/page.tsx');
files.push('./src/app/project/[id]/workspace/step2/page.tsx');
files.push('./src/app/project/[id]/workspace/step3/page.tsx');
files.push('./src/app/project/[id]/workspace/step4/page.tsx');
files.push('./src/app/project/[id]/workspace/step5/page.tsx');
files.push('./src/app/project/[id]/export/page.tsx');

files.forEach(file => {
  if (fs.existsSync(file)) {
    let content = fs.readFileSync(file, 'utf8');
    
    // Replace variants
    content = content.replace(/variant="bordered"/g, ''); // default is usually fine
    content = content.replace(/variant="flat"/g, 'variant="secondary"');
    content = content.replace(/variant="light"/g, 'variant="ghost"');
    content = content.replace(/variant="faded"/g, 'variant="secondary"');
    
    // Replace colors that might be invalid
    content = content.replace(/color="default"/g, 'color="secondary"');
    
    // Replace CheckboxGroup and Checkbox onValueChange
    content = content.replace(/onValueChange=/g, 'onChange=');
    
    // Fix CheckboxGroup onChange typing if needed, but standard is onChange
    
    fs.writeFileSync(file, content, 'utf8');
  }
});
console.log("Fixed!");
